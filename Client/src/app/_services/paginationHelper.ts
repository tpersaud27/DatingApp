import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/Pagination';

// We made this method more generic using T
export function getPaginatedResults<T>(
  url: string,
  params: HttpParams,
  http: HttpClient
) {
  const paginatedResult: PaginatedResult<T[]> = new PaginatedResult<T[]>();

  return (
    http
      // Doing this will allow us to get the response instead of just the http body
      .get<T[]>(url, { observe: 'response', params })
      .pipe(
        map((response) => {
          if (response.body) {
            // Remeber that result is the list of items we will get back, this will be the body of the request
            paginatedResult.result = response.body;
          }
          const pagination = response.headers.get('Pagination');
          if (pagination) {
            // We use JSON parse to convert the json to a object
            paginatedResult.pagination = JSON.parse(pagination);
          }
          return paginatedResult;
        })
      )
  );
}

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
  // HttpParams allows us to set query strings
  let params = new HttpParams();

  params = params.append('pageNumber', pageNumber);
  params = params.append('pageSize', pageSize);

  return params;
}
