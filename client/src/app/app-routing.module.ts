import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';



const routes: Routes = [
  { path: '', component: HomeComponent }, // '' indicates the route of 'localhost:4200'
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard], // Protected routes in here and children
    children: [
      { path: 'members', component: MemberListComponent },        // indicats the route of 'localhost:4200/members'
      { 
        path: 'members/:username', 
        component: MemberDetailComponent, 
        resolve: {member: MemberDetailedResolver}
      },  // indicats the route of 'localhost:4200/members/id'
      { path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard] },    // indicates the route of 'localhost:4200/member/edit'
      { path: 'lists', component: ListsComponent },               // indicats the route of 'localhost:4200/lists'
      { path: 'messages', component: MessagesComponent },         // indicats the route of 'localhost:4200/messages'
      { path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]},
    ]
  },
  { path: 'errors', component: TestErrorsComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' }, // '**' wildcard - all other routes 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
