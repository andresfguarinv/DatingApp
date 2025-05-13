import { Component, inject, OnInit } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagementComponent } from "../user-management/user-management.component";
import { HasRoleDirective } from '../../directives/has-role.directive';
import { PhotoManagementComponent } from "../photo-management/photo-management.component";
import { AdminService } from '../../services/admin.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [TabsModule, UserManagementComponent, HasRoleDirective, PhotoManagementComponent],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {
 
}
