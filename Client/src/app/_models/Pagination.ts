export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  // Remember T is the list of what ever results we are paginating
  // We will set the result to the list of items we get back
  result?: T;
  // the information we get in out header from the pagination will be set in this property
  pagination?: Pagination;
}
