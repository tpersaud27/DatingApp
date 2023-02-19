import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/Member';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  // We need to access to the member from the member edit component
  // Parent is the member edit component
  @Input() member: Member | undefined;

  constructor() {}

  ngOnInit(): void {}
}
