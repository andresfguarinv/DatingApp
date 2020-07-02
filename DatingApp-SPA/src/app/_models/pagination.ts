export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totlPages: number;
}

export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}
