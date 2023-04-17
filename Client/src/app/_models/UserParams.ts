// We are using a class here instead of an interface because we want to initalize some default values

import { User } from './User';

// This is helpful for some of the parameters we will pass to the backend
export class UserParams {
  gender: string;
  minAge: number = 18;
  maxAge: number = 99;
  pageNumber: number = 1;
  pageSize: number = 5;
  orderBy: string = 'lastActive';

  constructor(user: User) {
    this.gender = user.gender === 'female' ? 'male' : 'female';
  }
}
