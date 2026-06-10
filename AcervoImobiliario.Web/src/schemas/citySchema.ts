import { z } from 'zod';

export const cityFormSchema = z.object({
  name: z.string().trim().min(1, 'O nome da cidade é obrigatório.'),
  state: z
    .string()
    .trim()
    .min(1, 'O estado é obrigatório.')
    .length(2, 'O estado deve conter 2 letras.')
    .transform((value) => value.toUpperCase()),
});

export type CityFormValues = z.infer<typeof cityFormSchema>;
