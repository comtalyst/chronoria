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
        light_text: 'rgb(17, 24, 39)',
        light_text_l: 'rgb(107, 114, 128)',
        light_hl: '#FFAC06',
        light_hl_l: '#FFDC46',
        light_hl_subtext: '#FFFFFF',
        light_border: 'rgb(209, 213, 219)',
        light_disabled: 'rgb(156, 163, 175)'
      }
    },
  },
  plugins: [
    require('flowbite/plugin')
  ]
}
