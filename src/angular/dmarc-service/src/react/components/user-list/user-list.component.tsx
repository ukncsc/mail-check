import * as React from 'react';
import Spinner from 'react-md-spinner';

export default ({ userList, isLoading, error }) => (
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
              <th className="d-inline-block col-6 text-md-center">Email Address</th>
            </tr>
          </thead>
          <tbody>
            {userList &&
              userList.map((user) =>
                <tr>
                  <td className="d-inline-block col-6">
                    {user}
                  </td>
                  <td className="d-inline-block col-6 text-md-center">
                    a@b.com
                </td>
                </tr>
              )
            }
          </tbody>
        </table>
      )}
  </div>
);