import * as React from 'react';
import Spinner from 'react-md-spinner';

export default ({ domainList, isLoading, error }) => (
  <div>
    {isLoading && (
      <div className="row justify-content-center">
        <div className="col-sm-2 text-center" style={{ paddingTop: '150px' }}>
          <Spinner size="100" singleColor="#051D49" />
        </div>
      </div>
    )}
    {error && <span>ERROR {error.message}</span>}
    {!isLoading &&
      !error && (
        <table className="table">
          <thead>
            <tr>
              <th className="d-inline-block col-6">Name</th>
            </tr>
          </thead>
          <tbody>
            {domainList &&
              domainList.map((domain) =>
                <tr>
                  <td className="d-inline-block col-6">
                    {domain}
                  </td>
                </tr>
              )
            }
          </tbody>
        </table>
      )}
  </div>
);