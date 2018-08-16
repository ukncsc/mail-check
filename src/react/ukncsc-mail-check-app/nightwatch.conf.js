const chromedriver = require('chromedriver');
const seleniumServer = require('selenium-server');

module.exports = {
  src_folders: ['./test/e2e'],

  output_folder: './test/reports',

  custom_commands_path: '',

  test_workers: true,

  selenium: {
    start_process: true,
    server_path: seleniumServer.path,
    log_path: '',
    port: 4444,
    cli_args: {
      'webdriver.chrome.driver': chromedriver.path,
    },
  },

  test_settings: {
    default: {
      launch_url: 'https://www.staging.mailcheck.service.ncsc.gov.uk',
      selenium_host: 'ondemand.saucelabs.com',
      selenium_port: 4444,
      username: '${SAUCE_USERNAME}',
      access_key: '${SAUCE_ACCESS_KEY}',
      use_ssl: false,
      silent: true,
      output: true,
      screenshots: {
        enabled: false,
        on_failure: true,
        path: './test/screenshots',
      },
      desiredCapabilities: {
        javascriptEnabled: true,
        acceptSslCerts: true,
      },
      selenium: {
        start_process: false,
      },
      globals: {
        waitForConditionTimeout: 10000,
      },
    },

    'sauce-android': {
      desiredCapabilities: {
        name: 'Android Chrome',
        appiumVersion: '1.6.5',
        platformName: 'Android',
        platformVersion: '7.0',
        deviceName: 'Android GoogleAPI Emulator',
        browserName: 'chrome',
      },
      globals: {
        propertyData: {
          environment: 'android-7',
        },
      },
    },

    'sauce-iphone': {
      desiredCapabilities: {
        name: 'iOS Safari',
        appiumVersion: '1.7.1',
        platformName: 'iOS',
        platformVersion: '11.0',
        deviceName: 'iPhone X Simulator',
        browserName: 'Safari',
      },
      globals: {
        propertyData: {
          environment: 'ios-iphone',
        },
      },
    },

    'sauce-osx-chrome': {
      desiredCapabilities: {
        name: ' macOS Chrome',
        platform: 'macOS 10.12',
        browserName: 'chrome',
        version: '61',
      },
      globals: {
        propertyData: {
          environment: 'os_x_10_12-chrome61',
        },
      },
    },

    'sauce-osx-safari': {
      desiredCapabilities: {
        name: 'macOS Safari',
        platform: 'macOS 10.12',
        browserName: 'safari',
        version: '10.0',
      },
      globals: {
        propertyData: {
          environment: 'os_x_10_12-safari10',
        },
      },
    },

    'sauce-windows-firefox': {
      desiredCapabilities: {
        name: 'Windows Firefox',
        platform: 'Windows 10',
        browserName: 'Firefox',
        version: '47.0',
      },
      globals: {
        propertyData: {
          environment: 'windows10-firefox47',
        },
      },
    },

    'local-chrome': {
      desiredCapabilities: {
        name: 'Local Chrome',
        browserName: 'chrome',
      },
      selenium_host: 'localhost',
      selenium: {
        start_process: true,
      },
    },

    'local-firefox': {
      desiredCapabilities: {
        name: 'Local Firefox',
        browserName: 'firefox',
        marionette: true,
      },
      globals: {
        propertyData: {
          environment: 'local-firefox',
        },
      },
      selenium_host: 'localhost',
      selenium: {
        start_process: true,
      },
    },

    'local-safari': {
      desiredCapabilities: {
        name: 'Local Safari',
        browserName: 'safari',
      },
      globals: {
        propertyData: {
          environment: 'local-safari',
        },
      },
      selenium_host: 'localhost',
      selenium: {
        start_process: true,
      },
    },
  },

  MAIL_CHECK_TEST_USERNAME: '${MAIL_CHECK_TEST_USERNAME}',

  MAIL_CHECK_TEST_PASSWORD: '${MAIL_CHECK_TEST_PASSWORD}',
};
