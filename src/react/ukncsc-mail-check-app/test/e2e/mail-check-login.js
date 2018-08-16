const {
  MAIL_CHECK_TEST_USERNAME,
  MAIL_CHECK_TEST_PASSWORD,
} = require('../../nightwatch.conf');

module.exports = {
  'Demo test Mail Check login': browser => {
    browser
      .url('https://www.staging.mailcheck.service.ncsc.gov.uk')
      .waitForElementVisible('input#username')
      .setValue('input#username', MAIL_CHECK_TEST_USERNAME)
      .setValue('input#password', MAIL_CHECK_TEST_PASSWORD)
      .click('input[name=login]')
      .pause(1000)
      .assert.title('UK NCSC - Mail Check')
      .end();
  },
};
