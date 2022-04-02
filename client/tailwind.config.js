module.exports = {
  mode: 'jit',
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./node_modules/flowbite/**/*.js"
  ],
  theme: {
    fontFamily: {
      'sans': ['Open Sans', 'ui-sans-serif', 'system-ui']
    },
    extend: {
      colors: {
        light_bg: '#FFFFE0',
        light_text: '#000000',
        light_hl: '#FFAC06',
        light_hl_l: '#FFDC46',
        light_hl_subtext: '#FFFFFF'
      }
    },
  },
  plugins: [
    require('flowbite/plugin')
  ]
}
