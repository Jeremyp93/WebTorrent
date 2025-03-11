import { Routes } from "@angular/router";
import { ManagerComponent } from "./manager.component";
import { authGuard } from "../auth/auth.guard";
import { WebtorrentComponent } from "./webtorrent/webtorrent.component";

export const MANAGER_ROUTES: Routes = [
    {
        path: '', component: ManagerComponent, canActivate: [authGuard], canActivateChild: [authGuard], children: [
            { path: '', component: WebtorrentComponent, pathMatch: 'full', title: 'WebTorrent' },
        ]
    },
];