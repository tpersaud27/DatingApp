using AutoMapper;
using DatingApp.DAL.Interfaces;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using DatingApp.Services.Extensions;
using DatingApp.Services.Interfaces;
using DatingApp.Services.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        /// <summary>
        /// The client will send the request as a query string
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            // Get the current user
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // Add the current user to the userParams
            userParams.CurrentUsername = currentUser.UserName;

            // We want users to select a gender, if they do not select a gender we will return a default
            if(string.IsNullOrEmpty(userParams.Gender)) 
            {
                // Default we will just return the opposite gender
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            
            }

            var users = await _userRepository.GetMembersAsync(userParams);
            // Adding the pagination header to the response
            // This is so the client can access the pagination properties
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages));

            return Ok(users);

        }

        [HttpGet("id/{id}")] //api/users/id/{id}
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(userToReturn);


        }

        [HttpGet("username/{username}")] //api/users/{username}
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string userName)
        {
            return await _userRepository.GetMemberByUsernameAsync(userName);
        }


        /// <summary>
        /// Allows a user to update their profile information
        /// </summary>
        /// <param name="memberUpdateDto"></param>
        /// <returns> As per REST API standards for updating content we return a 204 response code </returns>
        [HttpPut()]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // This 'User' is from System.Security.Claim.Claims. 
            // In here we will get access to our token, which we can get the userName from
            var userName = User.GetUsername();
            // When we retrieve this user, it is being tracked by EF
            // Any changes will be tracked by EF
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            if (user == null) return NotFound();

            // We can use mapper to update the properties from out memberDto to our AppUser
            // This will override the properties in our user with those from memberDto
            // At this point nothing is saved
            _mapper.Map(memberUpdateDto, user);

            // This will return a status code of 204
            if (await _userRepository.SaveAllAsync()) return NoContent();

            // This would occur if the user did not make any changes 
            return BadRequest("Failed to update user");
        }

        /// <summary>
        /// Allows a user to add a new photo
        /// </summary>
        /// <param name="file"></param>
        /// <returns> As per REST API standards we should be returning a 201 response for creating resources </returns>
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // Getting the user using the JWT Claims
            var userName = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            // If there is no user 
            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            // If there is a error, return the error message
            if (result.Error != null) return BadRequest(result.Error.Message);

            // We want to add the photo to the user 
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,

            };


            // Check if this is the first photo the user is uploading
            // If so we want to set this as the main photo
            if (user.Photos.Count == 0) {
                photo.IsMain = true;
            }

            // Add the photo to the user list of photos
            user.Photos.Add(photo);

            // Since entity framework is tracking changes we can check if there any saved changes
            if (await _userRepository.SaveAllAsync())
            {
                // Return photoDto
                // We map into the PhotoDto from the photo
                // So the properties in PhotoDto are population from that of photo
                //return _mapper.Map<PhotoDto>(photo);

                // We are using this because we want to return a 201 response code as per REST api standards
                return CreatedAtAction(nameof(GetUserByUsername), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            // If the changes are not saved successfully return bad request
            return BadRequest("Problem Adding Photo");

        }

        /// <summary>
        /// Allows a user to update a new photo as their main photo
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns></returns>
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {

            // Remember we are getting the user name from the JWT claims
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user is null) return NotFound();

            // Get the photo based on the id 
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo is null) return NotFound();

            // We should check if the photo is already the main phot
            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            // Check if the currentMain is not null, this means there is already a photo there.
            if (currentMain != null)
            {
                // Set the current main to false
                currentMain.IsMain = false;
                // Set the new photo to main
                photo.IsMain = true;
            }

            // If the changes were saved
            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {

            // Getting the user using the JWT Claims
            var userName = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            // Find the photo based on the id
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound("Photo not found.");

            // Check if photo is main photo. 
            // Note: User should not be able to delete main photo
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");


            // If this is a cloudinary photo
            if (photo.PublicId != null)
            {
                // Call DeletePhotoAsync method from photoservice
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            // Remove the photo
            user.Photos.Remove(photo);

            // If the changes were saved return Ok
            if (await _userRepository.SaveAllAsync()) return Ok("Photo successfully deleted.");

            return BadRequest("Problem deleting photo.");


        }


    }
}
