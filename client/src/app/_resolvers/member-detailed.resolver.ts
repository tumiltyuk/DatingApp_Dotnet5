import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Member } from '../_models/Member';
import { MembersService } from '../_services/members.service';

@Injectable({
    providedIn: 'root'
})

// this provides a way of getting data ready before Angular constructs a page
// alternative means of preparing data than ng-IF
export class MemberDetailedResolver implements Resolve<Member> {

    constructor(private memberService: MembersService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
        
        return this.memberService.getMember(route.paramMap.get('username'));
    }

}