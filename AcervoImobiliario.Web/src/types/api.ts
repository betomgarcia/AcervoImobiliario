export enum ComplementType {
  None = 0,
  Apartment = 1,
  Room = 2,
  Store = 3,
  House = 4,
  Block = 5,
  Lot = 6,
  Floor = 7,
  ParkingSpace = 8,
  Other = 9,
}

export enum PropertyHistoryEventType {
  Sale = 1,
  Rental = 2,
  Visit = 3,
  Proposal = 4,
  Contract = 5,
  Maintenance = 6,
  Note = 7,
  RegistrationUpdate = 8,
  Correction = 9,
  Other = 99,
}

export interface ApiErrorResponse {
  success: false;
  message: string;
  errors: string[];
}

export interface CityResponse {
  id: string;
  name: string;
  nameNormalized: string;
  state: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface PropertyResponse {
  id: string;
  cityId: string;
  cityNameSnapshot: string;
  neighborhood: string;
  neighborhoodNormalized: string;
  street: string;
  streetNormalized: string;
  number: string;
  complementType: ComplementType;
  complementValue: string | null;
  complementValueNormalized: string | null;
  cadastralIndex: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreatePropertyRequest {
  cityId: string;
  neighborhood: string;
  street: string;
  number: string;
  complementType: ComplementType;
  complementValue?: string | null;
  cadastralIndex?: string | null;
}

export interface UpdatePropertyRequest {
  cityId: string;
  neighborhood: string;
  street: string;
  number: string;
  complementType: ComplementType;
  complementValue?: string | null;
  cadastralIndex?: string | null;
  isActive: boolean;
}

export interface PropertyHistoryResponse {
  id: string;
  propertyId: string;
  eventType: PropertyHistoryEventType;
  eventDate: string;
  description: string;
  createdAt: string;
}

export interface CreatePropertyHistoryRequest {
  eventType: PropertyHistoryEventType;
  eventDate: string;
  description: string;
}

export interface SearchPropertiesParams {
  cityId?: string;
  neighborhood?: string;
  street?: string;
  number?: string;
  complementType?: ComplementType;
  complementValue?: string;
  cadastralIndex?: string;
  includeInactive?: boolean;
}

export type HistorySortDirection = 'asc' | 'desc';
