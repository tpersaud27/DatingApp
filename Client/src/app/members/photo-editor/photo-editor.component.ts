import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';

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

  constructor(private accountService: AccountService) {
    // Getting the user
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(event: any) {
    this.hasBaseDropZoneOver = event;
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
