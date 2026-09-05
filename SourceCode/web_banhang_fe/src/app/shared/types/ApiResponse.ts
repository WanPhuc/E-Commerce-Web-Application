export interface ApiResponse<T> {
  status: number;
  code?: string;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  data: T[];
  page: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
