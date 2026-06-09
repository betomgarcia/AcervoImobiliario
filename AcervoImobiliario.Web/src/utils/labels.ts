import { ComplementType, PropertyHistoryEventType } from '@/types/api';

export const complementTypeLabels: Record<ComplementType, string> = {
  [ComplementType.None]: 'Sem complemento',
  [ComplementType.Apartment]: 'Apartamento',
  [ComplementType.Room]: 'Sala',
  [ComplementType.Store]: 'Loja',
  [ComplementType.House]: 'Casa',
  [ComplementType.Block]: 'Bloco',
  [ComplementType.Lot]: 'Lote',
  [ComplementType.Floor]: 'Andar',
  [ComplementType.ParkingSpace]: 'Vaga',
  [ComplementType.Other]: 'Outro',
};

export const historyEventTypeLabels: Record<PropertyHistoryEventType, string> = {
  [PropertyHistoryEventType.Sale]: 'Venda',
  [PropertyHistoryEventType.Rental]: 'Locação',
  [PropertyHistoryEventType.Visit]: 'Visita',
  [PropertyHistoryEventType.Proposal]: 'Proposta',
  [PropertyHistoryEventType.Contract]: 'Contrato',
  [PropertyHistoryEventType.Maintenance]: 'Manutenção',
  [PropertyHistoryEventType.Note]: 'Observação',
  [PropertyHistoryEventType.RegistrationUpdate]: 'Atualização cadastral',
  [PropertyHistoryEventType.Correction]: 'Correção',
  [PropertyHistoryEventType.Other]: 'Outro',
};

export const historyEventTypeColors: Record<PropertyHistoryEventType, string> = {
  [PropertyHistoryEventType.Sale]: '#2E7D32',
  [PropertyHistoryEventType.Rental]: '#1565C0',
  [PropertyHistoryEventType.Visit]: '#6A1B9A',
  [PropertyHistoryEventType.Proposal]: '#EF6C00',
  [PropertyHistoryEventType.Contract]: '#00838F',
  [PropertyHistoryEventType.Maintenance]: '#5D4037',
  [PropertyHistoryEventType.Note]: '#455A64',
  [PropertyHistoryEventType.RegistrationUpdate]: '#283593',
  [PropertyHistoryEventType.Correction]: '#C62828',
  [PropertyHistoryEventType.Other]: '#757575',
};
