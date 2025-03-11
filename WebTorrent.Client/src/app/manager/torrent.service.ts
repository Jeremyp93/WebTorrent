import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TorrentService {
  private apiUrl = environment.apiUrl;  // Replace with your API URL
  #http = inject(HttpClient);

  uploadTorrent = (file: File): Observable<any> => {
    const formData = new FormData();
    formData.append('file', file);
    return this.#http.post(`${this.apiUrl}/upload`, formData, {
      headers: { 'X-XSRF-TOKEN': this.#getCsrfToken() },
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            return { status: 'progress', progress: Math.round(100 * event.loaded / (event.total || 1)) };
          case HttpEventType.Response:
            return event.body;
          default:
            return `Unhandled event: ${event.type}`;
        }
      })
    );
  }

  getDownloadStatus = (filename: string): Observable<any> => {
    return this.#http.get(`${this.apiUrl}/status?filename=${filename}`);
  }

  downloadFile = (filename: string): Observable<Blob> => {
    return this.#http.get(`${this.apiUrl}/download?filename=${filename}`, { responseType: 'blob' });
  }

  #getCsrfToken = (): string => {
    return document.cookie
      .split('; ')
      .find(row => row.startsWith('XSRF-TOKEN='))
      ?.split('=')[1] || '';
  }
}