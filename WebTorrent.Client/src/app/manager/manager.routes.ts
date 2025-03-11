import { Routes } from "@angular/router";
import { ManagerComponent } from "./manager.component";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { authGuard } from "../auth/auth.guard";
import { NewEntryComponent } from "./new-entry/new-entry.component";

export const MANAGER_ROUTES: Routes = [
    {
        path: '', component: ManagerComponent, canActivate: [authGuard], canActivateChild: [authGuard], children: [
            { path: '', component: DashboardComponent, pathMatch: 'full', title: 'Dashboard' },
            {path: 'new', component: NewEntryComponent, title: 'New Entry'}
        ]
    },
];