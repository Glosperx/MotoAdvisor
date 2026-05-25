export interface AuthResponse {
  token: string;
  email: string;
  userName: string;
  roles: string[];
}

export interface User {
  userName: string;
  email: string;
  roles: string[];
}

export interface Brand {
  id: number;
  name: string;
  country?: string;
  logoUrl?: string;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface MotorcycleSummary {
  id: number;
  name: string;
  year: number;
  price: number;
  engine?: string;
  power?: string;
  horsepower: number;
  licenseCategory?: string;
  isBeginnerFriendly: boolean;
  brandId: number;
  brandName: string;
  categoryId: number;
  categoryName: string;
  mainImageUrl?: string;
}

export interface MotorcycleDetail {
  id: number;
  name: string;
  year: number;
  price: number;
  engine?: string;
  power?: string;
  horsepower: number;
  licenseCategory?: string;
  isBeginnerFriendly: boolean;
  description?: string;
  brand: Brand;
  category: Category;
  imageUrls: string[];
  averageRating?: number;
  reviewCount: number;
}

export interface Review {
  id: number;
  rating: number;
  content?: string;
  createdAt: string;
  userName: string;
  userId: string;
}

export interface Favorite {
  motorcycleId: number;
  name: string;
  year: number;
  price: number;
  brandName: string;
  mainImageUrl?: string;
}

export interface RecommendedMotorcycle {
  id: number;
  name: string;
  brandName: string;
  categoryName?: string;
  year: number;
  price: number;
  horsepower: number;
  mainImageUrl?: string;
  similarityScore: number;
}

export interface RecommendationResult {
  aiResponse: string;
  motorcycles: RecommendedMotorcycle[];
}
