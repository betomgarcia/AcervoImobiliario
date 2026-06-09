import { z } from 'zod';
import { ComplementType } from '@/types/api';

const complementTypesRequiringValue = [
  ComplementType.Apartment,
  ComplementType.Room,
  ComplementType.Store,
] as const;

export const propertyFormSchema = z
  .object({
    cityId: z.string().min(1, 'Selecione uma cidade.'),
    neighborhood: z.string().trim().min(1, 'O bairro é obrigatório.'),
    street: z.string().trim().min(1, 'A rua é obrigatória.'),
    number: z
      .string()
      .trim()
      .min(1, 'O número é obrigatório.')
      .regex(/^\d+$/, 'O número deve conter somente dígitos.'),
    complementType: z.nativeEnum(ComplementType),
    complementValue: z.string().trim().optional().nullable(),
    cadastralIndex: z.string().trim().optional().nullable(),
  })
  .superRefine((data, ctx) => {
    if (data.complementType === ComplementType.None && data.complementValue?.trim()) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Complemento deve ficar vazio quando o tipo for "Sem complemento".',
        path: ['complementValue'],
      });
    }

    if (
      complementTypesRequiringValue.includes(
        data.complementType as (typeof complementTypesRequiringValue)[number],
      ) &&
      !data.complementValue?.trim()
    ) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Informe o valor do complemento para o tipo selecionado.',
        path: ['complementValue'],
      });
    }
  });

export type PropertyFormValues = z.infer<typeof propertyFormSchema>;
