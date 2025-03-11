import { Routes } from "@angular/router";
import { AuthComponent } from "./auth.component";
import { LoginComponent } from "./login/login.component";
import { GoogleCallbackComponent } from "./callback/google-callback/google-callback.component";

export const AUTH_ROUTES: Routes = [
    {
        path: '', component: AuthComponent, children: [
            { path: '', redirectTo: `/manager`, pathMatch: 'full' },
            { path: 'login', component: LoginComponent, title: 'Auth - Login' },
            { path: 'google-callback', component: GoogleCallbackComponent, title: 'Redirecting...' },
        ]
    },
];