import 'whatwg-fetch';
import * as urljoin from 'url-join';

//const baseUrl = 'https://nice-zoology.glitch.me/api/';
const baseUrl = window.location.origin;

export default async (
  route,
  method = 'GET',
  mode = 'no-cors',
  body = {},
  headers = {
    Accept: 'application/json',
  }
) => {
  const response = await fetch(urljoin(baseUrl, route), {
    method,
    body,
    headers,
    credentials: 'same-origin'
  });

  const json = await response.json();

  if (response.status >= 400) {
    throw Error(json.message);
  }

  return json;
};
