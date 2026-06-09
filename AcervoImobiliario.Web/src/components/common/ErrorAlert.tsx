import { Alert, AlertTitle, Stack } from '@mui/material';

interface ErrorAlertProps {
  title?: string;
  message: string;
  errors?: string[];
}

export function ErrorAlert({ title = 'Não foi possível continuar', message, errors }: ErrorAlertProps) {
  const items = errors?.length ? errors : [message];

  return (
    <Alert severity="error" sx={{ borderRadius: 2 }}>
      <AlertTitle>{title}</AlertTitle>
      <Stack component="ul" spacing={0.5} sx={{ m: 0, pl: 2 }}>
        {items.map((item) => (
          <li key={item}>{item}</li>
        ))}
      </Stack>
    </Alert>
  );
}
