import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class RestService {
  constructor(
    private _http: HttpClient,
  ) { }

  get<T>(url: string, params?: {}) {
    return this._http.get<T>(url, {
      params,
      observe: 'response'
    });
  }

  create<T>(url: string, body: any) {
    return this._http.post<T>(url, body, {
      observe: 'response'
    });
  }

  update<T>(url: string, body: any) {
    return this._http.put<T>(url, body, {
      observe: 'response'
    });
  }

  delete<T>(url: string) {
    return this._http.delete<T>(url, {
      observe: 'response'
    });
  }
}
