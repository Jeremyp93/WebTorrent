import { Component, inject } from '@angular/core';
import { TorrentService } from '../torrent.service';
import { interval } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-webtorrent',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './webtorrent.component.html',
  styleUrl: './webtorrent.component.css'
})
export class WebtorrentComponent {
  #torrentService = inject(TorrentService)

  selectedFile: File | null = null;
  uploadProgress = -1;
  downloadProgress = -1;
  filename = '';


  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadFile() {
    if (!this.selectedFile) return;
    this.#torrentService.uploadTorrent(this.selectedFile).subscribe(event => {
      if (event.status === 'progress') {
        this.uploadProgress = event.progress;
      } else {
        alert('Upload completed! Downloading started...');
        this.uploadProgress = -1;
        this.startDownloadProgressTracking(this.selectedFile!.name);
      }
    });
  }

  startDownloadProgressTracking(filename: string) {
    interval(2000).subscribe(() => {
      this.#torrentService.getDownloadStatus(filename).subscribe(status => {
        this.downloadProgress = status.progress;
        if (status.progress === 100) {
          alert('Download complete! You can now download the file.');
        }
      });
    });
  }

  downloadFile() {
    this.#torrentService.downloadFile(this.filename).subscribe(blob => {
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = this.filename;
      link.click();
    });
  }
}
