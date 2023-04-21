import { Injectable } from '@angular/core';
import {
  Router,
  Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/Member';

@Injectable({
  providedIn: 'root',
})
export class MemberDetailResolver implements Resolve<Member> {
  constructor(private memberService: MembersService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberService.getMemberByUserName(
      route.paramMap.get('username')!
    );
  }
}
