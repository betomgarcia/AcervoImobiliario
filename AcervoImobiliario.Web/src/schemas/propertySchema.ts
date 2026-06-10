import { z } from 'zod';

export const propertyFormSchema = z.object({
  cityId: z.string().min(1, 'Selecione uma cidade.'),
  neighborhood: z.string().trim().min(1, 'O bairro é obrigatório.'),
  street: z.string().trim().min(1, 'A rua é obrigatória.'),
  number: z
    .string()
    .trim()
    .min(1, 'O número é obrigatório.')
    .regex(/^\d+$/, 'O número deve conter somente dígitos.'),
  complement: z.string().trim().optional().nullable(),
  cadastralIndex: z.string().trim().optional().nullable(),
});

export type PropertyFormValues = z.infer<typeof propertyFormSchema>;
