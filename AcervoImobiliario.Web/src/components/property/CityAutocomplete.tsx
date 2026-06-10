import { Autocomplete, TextField } from '@mui/material';
import { useEffect, useMemo, useRef, useState } from 'react';
import { useActiveCities } from '@/hooks/useCities';
import type { CityResponse } from '@/types/api';

function normalizeSearchTerm(term: string): string {
  return term
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}

function filterCitiesByTerm(cities: CityResponse[], term: string): CityResponse[] {
  const normalized = normalizeSearchTerm(term);
  if (!normalized) {
    return cities;
  }

  return cities.filter(
    (city) =>
      city.nameNormalized.includes(normalized) ||
      city.name.toLowerCase().includes(normalized) ||
      city.state.toLowerCase().includes(normalized),
  );
}

interface CityAutocompleteProps {
  value: string;
  onChange: (cityId: string, city: CityResponse | null) => void;
  label?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  error?: boolean;
  helperText?: string;
}

export function CityAutocomplete({
  value,
  onChange,
  label = 'Cidade',
  placeholder,
  required,
  disabled,
  error,
  helperText,
}: CityAutocompleteProps) {
  const { data: allCities = [], isLoading } = useActiveCities();
  const selectedCityRef = useRef<CityResponse | null>(null);
  const [inputValue, setInputValue] = useState('');

  const cityOptions = useMemo(() => {
    if (inputValue.trim().length < 2) {
      return allCities;
    }
    return filterCitiesByTerm(allCities, inputValue);
  }, [allCities, inputValue]);

  const resolveCity = (cityId: string): CityResponse | null => {
    if (!cityId) {
      return null;
    }

    return allCities.find((city) => city.id === cityId) ?? null;
  };

  const selectedCity =
    resolveCity(value) ??
    (selectedCityRef.current?.id === value ? selectedCityRef.current : null);

  useEffect(() => {
    if (!value) {
      selectedCityRef.current = null;
      setInputValue('');
      return;
    }

    const city = resolveCity(value);
    if (city) {
      selectedCityRef.current = city;
      setInputValue(`${city.name} — ${city.state}`);
    }
  }, [value, allCities]);

  return (
    <Autocomplete
      options={cityOptions}
      value={selectedCity}
      loading={isLoading}
      disabled={disabled}
      filterOptions={(options) => options}
      onChange={(_, city: CityResponse | null) => {
        selectedCityRef.current = city;
        onChange(city?.id ?? '', city);
        if (city) {
          setInputValue(`${city.name} — ${city.state}`);
        }
      }}
      inputValue={inputValue}
      onInputChange={(_, newValue, reason) => {
        if (reason === 'reset') {
          return;
        }

        setInputValue(newValue);

        if (reason === 'clear') {
          selectedCityRef.current = null;
          onChange('', null);
          return;
        }

        if (selectedCityRef.current) {
          const label = `${selectedCityRef.current.name} — ${selectedCityRef.current.state}`;
          if (newValue !== label) {
            selectedCityRef.current = null;
            onChange('', null);
          }
        }
      }}
      getOptionLabel={(option) => `${option.name} — ${option.state}`}
      isOptionEqualToValue={(option, current) => option.id === current.id}
      renderInput={(params) => (
        <TextField
          {...params}
          label={label}
          placeholder={placeholder}
          required={required}
          error={error}
          helperText={helperText}
        />
      )}
    />
  );
}
