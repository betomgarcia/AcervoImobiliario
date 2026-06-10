import { zodResolver } from '@hookform/resolvers/zod';
import { Box, Button, Grid, Stack, TextField } from '@mui/material';
import { Controller, useForm } from 'react-hook-form';
import { StatusChip } from '@/components/ui/StatusChip';
import { cityFormSchema, type CityFormValues } from '@/schemas/citySchema';

interface CityFormProps {
  defaultValues?: Partial<CityFormValues>;
  submitLabel: string;
  isSubmitting?: boolean;
  showStatus?: boolean;
  isActive?: boolean;
  onSubmit: (values: CityFormValues) => void;
  onCancel: () => void;
}

const defaultFormValues: CityFormValues = {
  name: '',
  state: 'MG',
};

export function CityForm({
  defaultValues,
  submitLabel,
  isSubmitting,
  showStatus,
  isActive,
  onSubmit,
  onCancel,
}: CityFormProps) {
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<CityFormValues>({
    resolver: zodResolver(cityFormSchema),
    defaultValues: { ...defaultFormValues, ...defaultValues },
  });

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
      <Grid container spacing={2}>
        <Grid item xs={12} md={8}>
          <Controller
            name="name"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                label="Nome da cidade"
                fullWidth
                required
                error={Boolean(errors.name)}
                helperText={errors.name?.message}
              />
            )}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <Controller
            name="state"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                label="Estado (UF)"
                fullWidth
                required
                inputProps={{ maxLength: 2, style: { textTransform: 'uppercase' } }}
                error={Boolean(errors.state)}
                helperText={errors.state?.message ?? 'Ex.: MG'}
              />
            )}
          />
        </Grid>
        {showStatus ? (
          <Grid item xs={12}>
            <StatusChip
              active={isActive ?? false}
              label={isActive ? 'Ativa' : 'Inativa'}
            />
          </Grid>
        ) : null}
      </Grid>

      <Stack direction={{ xs: 'column-reverse', sm: 'row' }} spacing={1.5} sx={{ mt: 3 }}>
        <Button variant="outlined" onClick={onCancel} disabled={isSubmitting}>
          Cancelar
        </Button>
        <Button type="submit" variant="contained" disabled={isSubmitting}>
          {submitLabel}
        </Button>
      </Stack>
    </Box>
  );
}
