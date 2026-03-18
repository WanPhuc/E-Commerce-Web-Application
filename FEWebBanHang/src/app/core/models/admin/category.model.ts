export interface CategoryDto {
    id: string;
    name: string;
    parentId?: string;
    children: CategoryDto[];
}
export interface CreateCategoryDto {
    name: string;
    parentId?: string;
}
export interface UpdateCategoryDto {
    name: string;
    parentId?: string;
}
export interface FlatOption {
  id: string;
  label: string;
  level: number;
}
