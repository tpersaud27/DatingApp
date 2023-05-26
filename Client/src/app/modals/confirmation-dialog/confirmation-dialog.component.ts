import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.css'],
})
export class ConfirmationDialogComponent implements OnInit {
  title = '';
  message = '';
  btnOk = '';
  btnCancelText = '';
  result = false;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  confirm() {
    this.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }
}
