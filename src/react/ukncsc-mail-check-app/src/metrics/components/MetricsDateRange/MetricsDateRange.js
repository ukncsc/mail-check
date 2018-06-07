import React from 'react';
import moment from 'moment';
import { DateRangePicker } from 'react-dates';
import { Button } from 'semantic-ui-react';

const quickDatesButtonConfig = [
  { value: 12, unit: 'M', display: 'Months' },
  { value: 6, unit: 'M', display: 'Months' },
  { value: 3, unit: 'M', display: 'Months' },
  { value: 1, unit: 'M', display: 'Month' },
];
export default ({
  startDate,
  endDate,
  focusedInput,
  setDateRange,
  setFocusedInput,
}) => (
  <React.Fragment>
    <div style={{ marginBottom: 20 }}>
      <Button.Group
        size="large"
        primary
        fluid
        buttons={quickDatesButtonConfig.map(
          ({ value, unit, display }, key) => ({
            content: `Last ${value} ${display}`,
            onClick: () =>
              setDateRange(moment().subtract(value, unit), moment()),
            key,
          })
        )}
      />
    </div>
    <div style={{ textAlign: 'center' }}>
      <DateRangePicker
        showDefaultInputIcon
        startDateId="metricsStartDate"
        endDateId="metricsEndDate"
        startDate={startDate}
        endDate={endDate}
        onDatesChange={dates => setDateRange(dates.startDate, dates.endDate)}
        onFocusChange={input => setFocusedInput(input)}
        focusedInput={focusedInput}
        isOutsideRange={d => d.isAfter(moment())}
        isDayBlocked={d => d.day() > 0}
        displayFormat="Do MMM YY"
      />
    </div>
  </React.Fragment>
);
