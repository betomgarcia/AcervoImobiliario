import { z } from 'zod';
import { PropertyHistoryEventType } from '@/types/api';

export const historyFormSchema = z.object({
  eventType: z.nativeEnum(PropertyHistoryEventType, {
    errorMap: () => ({ message: 'Selecione o tipo de evento.' }),
  }),
  eventDate: z.string().min(1, 'Informe a data do evento.'),
  description: z
    .string()
    .trim()
    .min(3, 'A descrição deve ter pelo menos 3 caracteres.')
    .max(2000, 'A descrição pode ter no máximo 2000 caracteres.'),
});

export type HistoryFormValues = z.infer<typeof historyFormSchema>;
