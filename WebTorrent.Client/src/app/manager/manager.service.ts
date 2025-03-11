import { inject, Injectable, signal, WritableSignal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  private apiUrl = environment.apiUrl;  // Replace with your API URL
  #http = inject(HttpClient);
  // Signal to store the data array
  data: WritableSignal<any[]> = signal([]);

  // getEntries(): Observable<Entry[]> {
  //   return this.#http.get<Entry[]>(`${this.apiUrl}/entries`)
  //     .pipe(
  //       tap((entries) => this.data.set(entries)),
  //       catchError(this.handleError<Entry[]>('getEntries'))
  //     );
  // }

  // // Method to add a new entry
  // addEntry(newEntry: Entry): Observable<Entry> {
  //   return this.#http.post<Entry>(`${this.apiUrl}/entries`, newEntry)
  //     .pipe(
  //       tap((createdEntry) => this.data.update((currentData) => [...currentData, createdEntry])),
  //       catchError(this.handleError<Entry>('addEntry'))
  //     );
  // }

  // Method to delete an entry by index
  deleteEntry(id: string): Observable<void> {
    return this.#http.delete<void>(`${this.apiUrl}/entries/${id}`)
    .pipe(
      tap(() => this.data.update((currentData) => currentData.filter((x) => x.id !== id))),
      catchError(this.handleError<void>('deleteEntry'))
    );
  }

  // // Method to update an entry by index
  // updateEntry(id: string, updatedEntry: Entry): Observable<void> {
  //   return this.#http.put<void>(`${this.apiUrl}/entries/${id}`, updatedEntry)
  //   .pipe(
  //     tap(() => this.data.update((currentData) => currentData.map((item) => (item.id === id ? { ...item, ...updatedEntry } : item)))),
  //     catchError(this.handleError<void>('updateEntry'))
  //   );
  // }

  // This method will trigger the download of the CSV file
  downloadCsv(): Observable<Blob> {
    const url = `${this.apiUrl}/entries/download`; // Adjust URL as needed
    return this.#http.get(url, { responseType: 'blob' });
  }

  // Error handler
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}