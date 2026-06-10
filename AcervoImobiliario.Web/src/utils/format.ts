import type { PropertyResponse } from '@/types/api';

export function formatAddress(
  street: string,
  number: string,
  neighborhood: string,
  cityName: string,
): string {
  return `${street}, ${number} — ${neighborhood}, ${cityName}`;
}

export function formatFullPropertyAddress(
  property: Pick<
    PropertyResponse,
    'street' | 'number' | 'neighborhood' | 'cityNameSnapshot' | 'complement'
  >,
): string {
  const base = formatAddress(
    property.street,
    property.number,
    property.neighborhood,
    property.cityNameSnapshot,
  );

  if (!property.complement?.trim()) {
    return base;
  }

  return `${base} — ${property.complement}`;
}

export function summarizeText(text: string, maxLength = 120): string {
  const normalized = text.trim().replace(/\s+/g, ' ');
  if (normalized.length <= maxLength) {
    return normalized;
  }
  return `${normalized.slice(0, maxLength).trimEnd()}…`;
}

export function formatDateTime(value: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

export function formatDate(value: string): string {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'long' }).format(new Date(value));
}

export function toIsoDateTimeLocal(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

export function fromIsoDateTimeLocal(value: string): string {
  return new Date(value).toISOString();
}
