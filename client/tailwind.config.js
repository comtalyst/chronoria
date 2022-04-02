module.exports = {
  mode: 'jit',
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
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
  plugins: [],
}
