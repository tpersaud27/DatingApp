using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities;
using DatingApp.Services.Extensions;

namespace DatingApp.DAL.AutomapperConfig
{
    public class AutomapperConfig: Profile
    {

        public AutomapperConfig()
        {
            // This is a map from AppUser to MemeberDto
            // This is how we configure the PhotoUrl property in MemberDto
            CreateMap<AppUser, MemberDto>()
                .ForMember(destination => destination.PhotoUrl,
                 options => options.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                // This will calculate the age for the person
                .ForMember(destination => destination.Age,
                options => options.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            // This will map the properties in MemberDto to AppUser entity
            CreateMap<MemberUpdateDto, AppUser>();
            // Making the register dto to app user so when a user registers they have the correct stored values
            CreateMap<RegisterDto, AppUser>();


            // This map the message properties into out messageDto
            // We need to notify Automapper of the Sender and Recipient PhotoUrl
            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(src => src.Sender.Photos
                    .FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(src => src.Recipient.Photos
                    .FirstOrDefault(x => x.IsMain).Url));

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? 
                DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);

        }
    }
}
