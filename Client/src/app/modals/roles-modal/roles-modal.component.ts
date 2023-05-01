import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css'],
})
export class RolesModalComponent implements OnInit {
  // We want to populate these properties from the user-management component
  // These will be passed through the openRolesModal method in the user-management component
  username: any;
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  // This is to update the value of if a checkbox is checked or not
  // based on which role the user is in
  updateChecked(checkedValue: string) {
    // If the index is -1, this means that the role was no in the selectedRoles
    // this means we want to add it to the selected roles, otherwise we remove it
    const index = this.selectedRoles.indexOf(checkedValue);

    index !== -1
      ? this.selectedRoles.splice(index, 1)
      : this.selectedRoles.push(checkedValue);
  }

  
}
