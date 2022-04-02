import logo from './media/logo.png';
import './App.css';

function App() {
  return (
    <div className='flex min-h-screen bg-light_bg text-light_text text-3xl'>
      <div className='flex-col px-20 py-12 w-1/2'>
        <div className='mb-20'>
          <span className='text-8xl font-black leading-snug'>
            Time Travel <br/> with <br/>
            <span className='text-light_hl'>
              Timelette!
            </span>
          </span>
        </div>
        <div className='mb-20 max-w-[38rem]'>
          <span className='text-4xl font-light leading-snug'>
            A message, a hidden treasure map, a confession, a memory, or whatever it is.
            Send it as <span className='text-light_hl font-bold'>a letter</span> to yourself or someone else from the future.
          </span>
        </div>
        <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                        hover:bg-light_hl_l hover:scale-105
                        transition-all duration-200'>
          Get Started
        </button>
      </div>
      <div className='px-20 py-20 w-1/2'>
        <img src={logo} alt='Timelette' className='hover:scale-95 transition-all duration-500'/>
      </div>
    </div>
  );
}

export default App;
