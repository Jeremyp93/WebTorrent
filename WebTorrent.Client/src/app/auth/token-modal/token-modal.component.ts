import { Component, input, output } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-token-modal',
  standalone: true,
  imports: [],
  templateUrl: './token-modal.component.html',
  styleUrl: './token-modal.component.css'
})
export class TokenModalComponent {
  token = input('');
  isVisible = input(false);
  closeModalEvent = output();
  url: string = `${environment.url}/auth/login?token=`;

  // Copy the token to clipboard
  copyToClipboard = (): void => {
    navigator.clipboard.writeText(this.token()).then(() => {
      alert('Token copied to clipboard!');
    });
  }

  // Copy the token to clipboard
  copyToClipboardUrl = (): void => {
    navigator.clipboard.writeText(`${environment.url}/auth/login?token=${this.token()}`).then(() => {
      alert('Url copied to clipboard!');
    });
  }

  // Emit event to parent to close the modal
  closeModal = (): void => {
    this.closeModalEvent.emit();
  }
}
