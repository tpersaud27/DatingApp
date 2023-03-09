import { MembersService } from 'src/app/_services/members.service';
import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/Photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  // We need to access to the member from the member edit component
  // Parent is the member edit component
  @Input() member: Member | undefined;

  // Ng2 File Uploader config
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;

  // We need access to the api so we can uploading the image
  baseUrl = environment.apiUrl;
  // We need to user
  user: User | undefined;

  constructor(
    private accountService: AccountService,
    private memberService: MembersService
  ) {
    // Getting the user
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }

  ngOnInit(): void {
    this.initializeUploader();

    console.log(this.member.photos);
  }

  fileOverBase(event: any) {
    this.hasBaseDropZoneOver = event;
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          // Set the new photoUrl for the user
          this.user.photoUrl = photo.url;
          // This is needed to update any other components that use the main photo (such as navbar)
          this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url;
          // This will access the photos and
          this.member.photos.forEach((p) => {
            // Set the current main to fasel
            if (p.isMain) p.isMain = false;
            // Set the new main
            if (p.id === photo.id) p.isMain = true;
          });
        }
      },
    });
  }

  /**
   * After we make the api call and pass in the photoId to be deleted, this will delete the photo from the database
   * We then need to delete the photo from the current stored photos for the member
   * @param photoId Photo Delete method
   */
  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      // Note: nothing is returned from this endpoint response
      next: () => {
        if (this.member) {
          // This will return all photos except for the one that matches the photoId to be deleted
          this.member.photos = this.member.photos.filter(
            (x) => x.id !== photoId
          );
        }
      },
    });
  }

  // Giving the file uploader some initial configurations
  initializeUploader() {
    this.uploader = new FileUploader({
      // Note: Since we are outside of the http request we must also specify the JWT Token in the header
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    };
  }
}
