import logo from './media/logo.png';
import './App.css';
import 'flowbite';

function App() {
  return (
    <div className='flex flex-col min-h-screen bg-light_bg text-light_text text-xl font-light'>
      <link rel="preconnect" href="https://fonts.googleapis.com"/>
      <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin='true'/>
      <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@300;400;700;800&display=swap" rel="stylesheet"></link>

      <div className='flex flex-wrap align-middle justify-around min-h-screen'>
        <div className='flex-col px-10 pt-12 w-fit'>
          <div className='mb-20'>
            <div className='text-5xl sm:text-8xl'>
              <span className=' font-black leading-snug'>
                Time Travel <br/> with <br/>
                <span className='text-light_hl'>
                  Timelette!
                </span>
              </span>
            </div>
          </div>
          <div className='mb-20 max-w-[34rem]'>
            <div className='text-2xl sm:text-3xl'>
              <span className='font-light leading-snug'>
                A message, a hidden treasure map, a confession, a memory, or whatever it is.
                Send it as <span className='text-light_hl font-bold'>a letter</span> to yourself or someone else from the future.
              </span>
            </div>
          </div>
          <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                          hover:bg-light_hl_l hover:scale-105
                          transition-all duration-200'>
            Get Started
          </button>
        </div>
        <div className='flex flex-col px-20 justify-around'>
          <img src={logo} alt='Timelette' className='w-[48rem] h-fit hover:scale-95 transition-all duration-500'/>
        </div>
      </div>

      <div className='flex flex-col min-h-screen px-6 md:px-24 pt-12'>
        <div className='flex flex-col mb-10'>
          <form>
            <div className='flex flex-wrap gap-y-4 justify-between mb-10'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderEmail'>
                  Sender's Email (yes, yours!)
                </label>
                <input id='senderEmail' type='email' placeholder='you@example.com' required
                className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderName'>
                  Sender's Alias
                </label>
                <input id='senderName' type='text' placeholder='Full name, nickname, or any identifier!' required
                className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>
            
            <div className='flex mb-3 px-4 items-center'>
              <input id='selfSend' aria-describedby='selfSend' type='checkbox' className='w-4 h-4 rounded border border-gray-300 focus:ring-3 focus:ring-light_hl ' required/>
              <label htmlFor='selfSend' className='text-base pl-3'>This letter is for myself</label>
            </div>

            <div className='flex flex-wrap gap-y-4 justify-between mb-10'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientEmail'>
                  Recipient's Email &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </label>
                <input id='recipientEmail' type='email' placeholder='they@example.com' required
                  className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientName'>
                  Recipient's Alias
                </label>
                <input id='recipientName' type='text' placeholder='How would you call this individual?' required
                className='border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>

            <div className='flex flex-col mb-10 space-y-3 px-4'>
              <label htmlFor='textContent'>
                Inside the Envelope
              </label>
              <textarea id='textContent' rows='5' placeholder='Hello...? (max 10,000 characters)' required
                className='w-full border border-gray-300 text-gray-900 rounded focus:ring-light_hl focus:border-light_hl'/>
              <input className='text-sm text-gray-900 bg-white rounded border border-gray-300 cursor-pointer 
                focus:outline-none focus:border-transparent focus:ring-2 focus:ring-light_hl' id='fileUpload' type='file'></input>
              <label htmlFor='fileUpload' className='text-base'>File upload is optional. The maximum allowed file size is 200 MB. Recommended Extensions: .pdf, .txt, .jpg, .mp4, .zip</label>
            </div>

            <div className='flex flex-col mb-10 space-y-3'>
              <div className='flex flex-wrap gap-y-4 justify-between'>
                <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                  <label htmlFor='sendDateRaw'>
                    Destination Date
                  </label>
                  <input type='date' className='text-gray-900 bg-white rounded border border-gray-300 focus:ring-light_hl focus:border-light_hl' placeholder='Select date'/>
                </div>
                <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                  <label htmlFor='sendTimeRaw'>
                    Destination Time
                  </label>
                  <input type='time' className='text-gray-900 bg-white rounded border border-gray-300 focus:ring-light_hl focus:border-light_hl' placeholder='Select date'/>
                </div>
              </div>
              <label htmlFor='sendDateRaw' className='px-4 -mt-10 text-base'>Date and time is based on your local time zone.</label>
            </div>

            

            <div className='flex flex-col mb-10 space-y-3 px-4'>
              <span className='text-base'>By clicking submit, you agree to the <a className='text-light_hl underline' href='https://google.com'>terms and conditions</a>.</span>
              <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded
                                 hover:bg-light_hl_l hover:scale-105
                                 transition-all duration-200'
                      type='submit'>
                Submit
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default App;
