import * as React from 'react';

export default ({page, pageSize, collectionSize, entityType}) => {
  let minRecord = ((page - 1) * pageSize) + 1;
  let maxRecord = ((minRecord + pageSize) - 1) <= collectionSize ? ((minRecord + pageSize) - 1) : collectionSize;

  return (
    <table>
      <tbody>
        <tr>
          <td className="text-md-center"><h2>{ minRecord }-{ maxRecord }</h2></td>
        </tr>
        <tr>
          <td className="text-md-center"><h5>of</h5></td>
        </tr>
        <tr>
          <td className="text-md-center"><h2>{ collectionSize }</h2></td>
        </tr>
        <tr>
          <td className="text-md-center"><h5>{ entityType }</h5></td>
        </tr>
      </tbody>
    </table>
  )
}