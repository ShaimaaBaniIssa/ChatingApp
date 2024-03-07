export interface Pagination {
  //same properties that return from Api respose (PaginationHeaders)
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T>{
  result?: T; // from body
  pagination?: Pagination; //from header
}
