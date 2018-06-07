import React from 'react';
import { VictoryChart } from 'victory';

export default props => (
  <div
    style={{
      position: 'relative',
      height: 0,
      width: '100%',
      padding: 0,
      paddingBottom: `${100 * (props.height / props.width)}%`,
    }}
  >
    <VictoryChart
      {...props}
      style={{
        ...props.style,
        parent: {
          position: 'absolute',
          height: '100%',
          width: '100%',
          left: 0,
          top: 0,
        },
      }}
    />
  </div>
);
