import { zodResolver } from '@hookform/resolvers/zod';
import HomeWorkIcon from '@mui/icons-material/HomeWork';
import {
  Autocomplete,
  Box,
  Button,
  Grid,
  Stack,
  TextField,
} from '@mui/material';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { CityAutocomplete } from '@/components/property/CityAutocomplete';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';
import {
  useSearchNeighborhoods,
  useSearchNumbers,
  useSearchStreets,
} from '@/hooks/useProperties';
import { propertyFormSchema, type PropertyFormValues } from '@/schemas/propertySchema';

interface PropertyFormProps {
  defaultValues?: Partial<PropertyFormValues>;
  submitLabel: string;
  isSubmitting?: boolean;
  onSubmit: (values: PropertyFormValues) => void;
}

const defaultFormValues: PropertyFormValues = {
  cityId: '',
  neighborhood: '',
  street: '',
  number: '',
  complement: '',
  cadastralIndex: '',
};

export function PropertyForm({
  defaultValues,
  submitLabel,
  isSubmitting,
  onSubmit,
}: PropertyFormProps) {
  const {
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<PropertyFormValues>({
    resolver: zodResolver(propertyFormSchema),
    defaultValues: { ...defaultFormValues, ...defaultValues },
  });

  const cityId = watch('cityId');
  const neighborhood = watch('neighborhood');
  const street = watch('street');

  const [neighborhoodInput, setNeighborhoodInput] = useState('');
  const debouncedNeighborhoodInput = useDebouncedValue(neighborhoodInput);
  const [streetInput, setStreetInput] = useState('');
  const debouncedStreetInput = useDebouncedValue(streetInput);
  const [numberInput, setNumberInput] = useState('');
  const debouncedNumberInput = useDebouncedValue(numberInput);

  const { data: neighborhoodOptions = [] } = useSearchNeighborhoods(
    cityId,
    debouncedNeighborhoodInput,
  );

  const { data: streetOptions = [] } = useSearchStreets(
    cityId,
    neighborhood,
    debouncedStreetInput,
  );

  const { data: numberOptions = [] } = useSearchNumbers(
    cityId,
    neighborhood,
    street,
    debouncedNumberInput,
  );

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
      <Grid container spacing={2.5}>
        <Grid item xs={12} md={6}>
          <Controller
            name="cityId"
            control={control}
            render={({ field }) => (
              <CityAutocomplete
                value={field.value}
                onChange={(nextCityId) => {
                  field.onChange(nextCityId);
                  setValue('neighborhood', '');
                  setValue('street', '');
                  setValue('number', '');
                }}
                placeholder="Selecione a cidade"
                required
                error={Boolean(errors.cityId)}
                helperText={errors.cityId?.message}
              />
            )}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <Controller
            name="neighborhood"
            control={control}
            render={({ field }) => (
              <Autocomplete
                freeSolo
                options={neighborhoodOptions}
                value={field.value}
                onChange={(_, value) => field.onChange(typeof value === 'string' ? value : value ?? '')}
                inputValue={neighborhoodInput}
                onInputChange={(_, value) => {
                  setNeighborhoodInput(value);
                  field.onChange(value);
                }}
                disabled={!cityId}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Bairro"
                    placeholder="Informe o bairro"
                    required
                    error={Boolean(errors.neighborhood)}
                    helperText={errors.neighborhood?.message}
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <Controller
            name="street"
            control={control}
            render={({ field }) => (
              <Autocomplete
                freeSolo
                options={streetOptions}
                value={field.value}
                onChange={(_, value) => field.onChange(typeof value === 'string' ? value : value ?? '')}
                inputValue={streetInput}
                onInputChange={(_, value) => {
                  setStreetInput(value);
                  field.onChange(value);
                }}
                disabled={!cityId || !neighborhood}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Rua"
                    placeholder="Informe a rua"
                    required
                    error={Boolean(errors.street)}
                    helperText={errors.street?.message}
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <Controller
            name="number"
            control={control}
            render={({ field }) => (
              <Autocomplete
                freeSolo
                options={numberOptions}
                value={field.value}
                onChange={(_, value) => field.onChange(typeof value === 'string' ? value : value ?? '')}
                inputValue={numberInput}
                onInputChange={(_, value) => {
                  setNumberInput(value);
                  field.onChange(value);
                }}
                disabled={!cityId || !neighborhood || !street}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Número"
                    placeholder="Apenas números"
                    required
                    error={Boolean(errors.number)}
                    helperText={errors.number?.message ?? 'Somente dígitos'}
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid item xs={12}>
          <Controller
            name="complement"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ''}
                fullWidth
                label="Complemento (opcional)"
                placeholder="Ex.: Apto 303 Bloco A, Loja 02, Casa fundos..."
              />
            )}
          />
        </Grid>

        <Grid item xs={12}>
          <Controller
            name="cadastralIndex"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ''}
                fullWidth
                label="Índice cadastral (opcional)"
                placeholder="Informe o índice cadastral"
              />
            )}
          />
        </Grid>
      </Grid>

      <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="flex-end" sx={{ mt: 3 }}>
        <Button
          type="submit"
          variant="contained"
          color="primary"
          size="large"
          disabled={isSubmitting}
          startIcon={<HomeWorkIcon />}
          fullWidth={false}
          sx={{ minWidth: { sm: 220 } }}
        >
          {isSubmitting ? 'Salvando...' : submitLabel}
        </Button>
      </Stack>
    </Box>
  );
}
