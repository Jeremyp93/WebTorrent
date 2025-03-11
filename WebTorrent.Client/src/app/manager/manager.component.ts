import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ManagerService } from './manager.service';

@Component({
  selector: 'app-manager',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './manager.component.html',
  styleUrl: './manager.component.css'
})
export class ManagerComponent {
  #managerService = inject(ManagerService);

  ngOnInit(): void {
    this.#managerService.getEntries().subscribe();
  }
}
