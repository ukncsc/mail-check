import React from 'react';
import { Header } from 'semantic-ui-react';

export default ({ page, pageSize, collectionSize }) => {
  const minRecord = collectionSize > 0 ? (page - 1) * pageSize + 1 : 0;
  const maxRecord =
    minRecord + pageSize - 1 <= collectionSize
      ? minRecord + pageSize - 1
      : collectionSize;
  return (
    <table>
      <tbody>
        <tr>
          <td>
            <Header as="h2" textAlign="center">
              {minRecord}-{maxRecord}
            </Header>
          </td>
        </tr>
        <tr>
          <td>
            <Header as="h5" textAlign="center">
              of
            </Header>
          </td>
        </tr>
        <tr>
          <td>
            <Header as="h2" textAlign="center">
              {collectionSize}
            </Header>
          </td>
        </tr>
        <tr>
          <td>
            <Header as="h5" textAlign="center">
              Domains
            </Header>
          </td>
        </tr>
      </tbody>
    </table>
  );
};
