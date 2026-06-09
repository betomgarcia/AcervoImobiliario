import { zodResolver } from '@hookform/resolvers/zod';
import {
  Autocomplete,
  Box,
  Button,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from '@mui/material';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useActiveCities, useSearchCities } from '@/hooks/useCities';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';
import {
  useSearchNeighborhoods,
  useSearchNumbers,
  useSearchStreets,
} from '@/hooks/useProperties';
import { propertyFormSchema, type PropertyFormValues } from '@/schemas/propertySchema';
import { ComplementType, type CityResponse } from '@/types/api';
import { complementTypeLabels } from '@/utils/labels';

interface PropertyFormProps {
  defaultValues?: Partial<PropertyFormValues>;
  submitLabel: string;
  isSubmitting?: boolean;
  onSubmit: (values: PropertyFormValues) => void;
}

const complementOptions = Object.entries(complementTypeLabels).map(([value, label]) => ({
  value: Number(value) as ComplementType,
  label,
}));

const defaultFormValues: PropertyFormValues = {
  cityId: '',
  neighborhood: '',
  street: '',
  number: '',
  complementType: ComplementType.None,
  complementValue: '',
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

  const complementType = watch('complementType');
  const cityId = watch('cityId');
  const neighborhood = watch('neighborhood');
  const street = watch('street');

  const [cityInput, setCityInput] = useState('');
  const debouncedCityInput = useDebouncedValue(cityInput);
  const [neighborhoodInput, setNeighborhoodInput] = useState('');
  const debouncedNeighborhoodInput = useDebouncedValue(neighborhoodInput);
  const [streetInput, setStreetInput] = useState('');
  const debouncedStreetInput = useDebouncedValue(streetInput);
  const [numberInput, setNumberInput] = useState('');
  const debouncedNumberInput = useDebouncedValue(numberInput);

  const { data: allCities = [] } = useActiveCities();
  const { data: searchedCities = [] } = useSearchCities(debouncedCityInput);

  const cityOptions = debouncedCityInput.trim().length >= 2 ? searchedCities : allCities;

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

  const requiresComplementValue = [
    ComplementType.Apartment,
    ComplementType.Room,
    ComplementType.Store,
  ].includes(complementType);

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
      <Grid container spacing={2}>
        <Grid item xs={12} md={6}>
          <Controller
            name="cityId"
            control={control}
            render={({ field }) => {
              const selectedCity =
                cityOptions.find((city) => city.id === field.value) ?? null;

              return (
                <Autocomplete
                  options={cityOptions}
                  value={selectedCity}
                  onChange={(_, city: CityResponse | null) => {
                    field.onChange(city?.id ?? '');
                    setValue('neighborhood', '');
                    setValue('street', '');
                    setValue('number', '');
                  }}
                  inputValue={cityInput}
                  onInputChange={(_, value) => setCityInput(value)}
                  getOptionLabel={(option) => `${option.name} — ${option.state}`}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Cidade"
                      required
                      error={Boolean(errors.cityId)}
                      helperText={errors.cityId?.message}
                    />
                  )}
                />
              );
            }}
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
                    required
                    error={Boolean(errors.number)}
                    helperText={errors.number?.message ?? 'Somente dígitos'}
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid item xs={12} md={4}>
          <Controller
            name="complementType"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth error={Boolean(errors.complementType)}>
                <InputLabel>Tipo de complemento</InputLabel>
                <Select
                  {...field}
                  label="Tipo de complemento"
                  onChange={(event) => {
                    const value = Number(event.target.value) as ComplementType;
                    field.onChange(value);
                    if (value === ComplementType.None) {
                      setValue('complementValue', '');
                    }
                  }}
                >
                  {complementOptions.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                      {option.label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            )}
          />
        </Grid>

        <Grid item xs={12} md={8}>
          <Controller
            name="complementValue"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ''}
                fullWidth
                label="Valor do complemento"
                disabled={!requiresComplementValue}
                error={Boolean(errors.complementValue)}
                helperText={errors.complementValue?.message}
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
              />
            )}
          />
        </Grid>
      </Grid>

      <Stack direction="row" justifyContent="flex-end" sx={{ mt: 3 }}>
        <Button type="submit" variant="contained" size="large" disabled={isSubmitting}>
          {isSubmitting ? 'Salvando...' : submitLabel}
        </Button>
      </Stack>
    </Box>
  );
}
