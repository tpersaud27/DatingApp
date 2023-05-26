import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ConfirmationDialogComponent } from '../modals/confirmation-dialog/confirmation-dialog.component';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ConfirmationService {
  bsModalRef?: BsModalRef<ConfirmationDialogComponent>;

  constructor(private modalService: BsModalService) {}

  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this?',
    btnOk = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message,
        btnOk,
        btnCancelText,
      },
    };
    this.bsModalRef = this.modalService.show(
      ConfirmationDialogComponent,
      config
    );
    return this.bsModalRef.onHidden!.pipe(
      map(() => {
        return this.bsModalRef!.content!.result;
      })
    );
  }
}
