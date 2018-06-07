import download from 'downloadjs';
import * as urljoin from 'url-join';

const mailCheckOIDCHeaders = () =>
  process.env.NODE_ENV === 'development'
    ? {
        [process.env.REACT_APP_OIDC_CLAIM_FAMILY_NAME_KEY]:
          process.env.REACT_APP_OIDC_CLAIM_FAMILY_NAME_VALUE,
        [process.env.REACT_APP_OIDC_CLAIM_NAME_KEY]:
          process.env.REACT_APP_OIDC_CLAIM_NAME_VALUE,
        [process.env.REACT_APP_OIDC_CLAIM_EMAIL_KEY]:
          process.env.REACT_APP_OIDC_CLAIM_EMAIL_VALUE,
      }
    : {};

export const apiFetch = baseUrl => async (endpoint, method = 'GET', body) => {
  const response = await fetch(urljoin(baseUrl, endpoint), {
    method,
    body: body && JSON.stringify(body),
    headers: new Headers({
      'Content-Type': 'application/json',
      ...mailCheckOIDCHeaders(),
    }),
    credentials: 'same-origin',
  });

  if (response.status === 401) {
    return window.location.replace('/');
  }

  if (response.status === 204) {
    return null;
  }

  let json;
  try {
    json = await response.json();
  } catch (err) {
    if (response.status === 403) {
      throw Error('403 Forbidden');
    }

    throw Error('Bad response from server');
  }

  if (response.status >= 400) {
    throw Error(json.message);
  }

  return json;
};

export const apiDownload = baseUrl => async (endpoint, fileName) => {
  const response = await fetch(urljoin(baseUrl, endpoint), {
    headers: new Headers({
      ...mailCheckOIDCHeaders(),
    }),
    credentials: 'same-origin',
  });

  if (response.status === 401) {
    return window.location.replace('/');
  }

  if (response.status === 403) {
    throw Error('You are not authorised to download this file.');
  }

  if (response.status >= 400) {
    throw Error('Sorry, there was a problem downloading the file.');
  }

  const blob = await response.blob();

  return download(blob, fileName);
};

export const mailCheckApiFetch = apiFetch(
  process.env.REACT_APP_API_ENDPOINT || `${window.location.origin}/api`
);

export const mailCheckApiDownload = apiDownload(
  process.env.REACT_APP_API_ENDPOINT || `${window.location.origin}/api`
);
