import React, {useState, useRef} from 'react';
import 'flowbite';
import Modal from 'flowbite/src/components/modal';
import _ from 'lodash';
import axios from 'axios';

import config from './config.json';
import dialogues from './dialogues.json';
import logo from './media/logo.png';

function App() {
  // States for form inputs
  const [selfSend, setSelfSend]                     = useState(false);
  const [senderEmailRaw, setSenderEmailRaw]         = useState('');
  const [senderNameRaw, setSenderNameRaw]           = useState('');
  const [recipientEmailRaw, setRecipientEmailRaw]   = useState('');
  const [recipientNameRaw, setRecipientNameRaw]     = useState('');
  const [textRaw, setTextRaw]                       = useState('');
  const [fileRaw, setFileRaw]                       = useState(null);
  const [sendTimeRaw, setSendTimeRaw]               = useState(null);
  
  // States for UI elements
  const [submitting, setSubmitting]                 = useState(false);
  const [currentWarning, setCurrentWarning]         = useState('');
  const [currentError, setCurrentError]             = useState('');
  const [completeModal, setCompleteModal]           = useState(null);

  // States for backend control
  const [lastSub, setLastSub] = useState(0);

  // References for UI elements
  const createFormRef = useRef(0);

  // Submission service
  const trueSubmit = async (e) => {
    e.preventDefault();
    // clean params
    const senderEmail = senderEmailRaw.trim();
    const senderName = senderNameRaw.trim();
    const [recipientEmail, recipientName] = (() => {
      if(selfSend){
        return [senderEmail, senderName];
      }
      else{
        return [recipientEmailRaw.trim(), recipientNameRaw.trim()];
      }
    })();
    const text = textRaw.trim();
    const sendTime = Date.parse(sendTimeRaw);

    // filter illegal params
    if(sendTime < Date.now()){
      setCurrentWarning(dialogues.warning.PAST_SEND_TIME);
      return false;
    }
    else if(sendTime < Date.now() + config.policies.minCapsuleAgeDays*24*60*60*1000){
      setCurrentWarning(dialogues.warning.TOO_EARLY_SEND_TIME);
      return false;
    }

    // type: text only
    if(fileRaw == null){
      // construct request body as json
      const reqBody = {
        senderEmail, senderName, recipientEmail, recipientName, sendTime, text
      }
      await axios.post(config.urls.webAPI + '/submit/text/', reqBody).catch((e) => {throw new Error(e.response.data)});
    }
    // type: file
    else{
      if(fileRaw.size > config.policies.maxFileSizeMB*1000000){
        setCurrentWarning(dialogues.warning.TOO_LARGE_FILE);
        return false;
      }

      // construct request body as form (for file)
      const reqBody = {
        senderEmail, senderName, recipientEmail, recipientName, sendTime, text
      }
      reqBody.textLocation = 'Before';              // not implementing this for now
      const formData = new FormData();
      for (const [key, val] of Object.entries(reqBody)){
        formData.append(key, val);
      }
      formData.append('FormFile', fileRaw);
      formData.append('FileName', fileRaw.name);
      await axios.post(config.urls.webAPI + '/submit/file', formData);
    }
    return true;
  }
  // Submission control
  const submit = async (e) => {
    e.preventDefault();
    setSubmitting(true);     // not safe: if user clicks faster than this line's execution, then it will be duped, but it happens rarely;
    setCurrentError('');
    setCurrentWarning('');

    // check if too frequent
    if(Date.now() - lastSub < config.policies.submitCooldown){
      setCurrentWarning(dialogues.warning.TOO_MANY_SUBMISSIONS);
      setSubmitting(false);
      return false;
    }
    
    // try sending request
    try{
      const res = await trueSubmit(e);
      if(!res){
        setSubmitting(false);
        return false;
      }
    } catch (e){
      if(dialogues.error[e.message] != null){
        setCurrentError(dialogues.error[e.message]);
      }
      else{
        setCurrentError(e.message);
      }
      setSubmitting(false);
      return false;
    }
    // complete
    setLastSub(Date.now());
    const modal = new Modal(document.getElementById('complete'));
    modal.show();
    setCompleteModal(modal);

    setSubmitting(false);
    return true;
  }
  const closeComplete = () => {
    completeModal.hide();
    setCompleteModal(null);
  }
  const getStarted = () => {
    window.scrollTo({top: createFormRef.current.offsetTop, behavior: 'smooth'});
  }
  
  return (
    <div className='flex flex-col min-h-screen bg-light_bg text-light_text text-lg'>
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
                Send it as <span className='text-light_hl font-bold'>a letter</span> to yourself or someone else from the future!
              </span>
            </div>
          </div>
          <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded-lg
                          hover:bg-light_hl_l hover:scale-105
                          transition-all duration-200' onClick={getStarted}>
            Get Started
          </button>
        </div>
        <div className='flex flex-col px-20 justify-around'>
          <img src={logo} alt='Timelette' className='w-[48rem] h-fit hover:scale-95 transition-all duration-500'/>
        </div>
      </div>

      <div ref={createFormRef} className='flex flex-col min-h-screen px-6 md:px-24 pt-12'>
        <div className='flex flex-col mb-8'>
          
          <form onSubmit={submit}>
            <div className='flex flex-wrap gap-y-4 justify-between mb-8'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderEmail'>
                  Sender's Email (yes, yours!)
                </label>
                <input id='senderEmail' type='email' placeholder='you@example.com' required maxLength={config.policies.maxEmailCh} onChange={(e) => setSenderEmailRaw(e.target.value)}
                className='border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='senderName'>
                  Sender's Alias
                </label>
                <input id='senderName' type='text' placeholder='Full name, nickname, or any identifier!' required maxLength={config.policies.maxNameCh} onChange={(e) => setSenderNameRaw(e.target.value)}
                className='border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>
            
            <div className='flex mb-3 px-4 items-center'>
              <input id='selfSend' aria-describedby='selfSend' type='checkbox' onChange={() => setSelfSend(!selfSend)}
                className='w-4 h-4 rounded border border-light_border focus:ring-3 focus:ring-light_hl '/>
              <label htmlFor='selfSend' className='text-sm pl-3'>This letter is for myself</label>
            </div>

            {(!selfSend)? (
            <div className='flex flex-wrap gap-y-4 justify-between mb-8'>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientEmail'>
                  Recipient's Email &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </label>
                <input id='recipientEmail' type='email' placeholder='they@example.com' required maxLength={config.policies.maxEmailCh} onChange={(e) => setRecipientEmailRaw(e.target.value)}
                  className='border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl'/>
              </div>
              <div className='flex flex-col min-w-[50%] grow space-y-3 px-4'>
                <label htmlFor='recipientName'>
                  Recipient's Alias
                </label>
                <input id='recipientName' type='text' placeholder='How would you call this individual?' required maxLength={config.policies.maxNameCh} onChange={(e) => setRecipientNameRaw(e.target.value)}
                  className='border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl'/>
              </div>
            </div>
            ):(<div/>)}

            <div className='flex flex-col mb-8 space-y-3 px-4'>
              <label htmlFor='text'>
                Inside the Envelope
              </label>
              <textarea id='text' rows='5' placeholder={'Hello...? (max ' + config.policies.maxTextCh + ' characters)'} required maxLength={config.policies.maxTextCh} onChange={_.debounce((ev) => setTextRaw(ev.target.value), 200)}
                className='w-full border border-light_border  rounded-lg focus:ring-light_hl focus:border-light_hl'/>
              <input className='text-sm  bg-white rounded-lg border border-light_border cursor-pointer 
                focus:outline-none focus:border-transparent focus:ring-2 focus:ring-light_hl' id='fileUpload' type='file' onChange={(e) => setFileRaw(e.target.files[0])}/>
              <label htmlFor='fileUpload' className='text-sm text-light_text_l'>File upload is optional. The maximum allowed file size is {config.policies.maxFileSizeMB} MB. Recommended extensions: .pdf, .txt, .jpg, .mp4, .zip</label>
            </div>

            <div className='flex flex-col mb-8 space-y-3'>
              <div className='flex flex-col max-w-[18rem] space-y-3 px-4'>
                <label htmlFor='sendTimeRaw'>
                  Destination Time
                </label>
                <input type='datetime-local' className=' bg-white rounded-lg border border-light_border focus:ring-light_hl focus:border-light_hl' 
                  placeholder='Select date' required onChange={(e) => setSendTimeRaw(e.target.value)}/>
              </div>
              <label htmlFor='sendTimeRaw' className='px-4 -mt-10 text-sm text-light_text_l'>Date and time is based on your local machine time zone. It needs to be at least 2 days from now.</label>
            </div>

            { (currentError || currentWarning)? (<div/>) : (
                <div className='p-4 mx-4 mb-4 text-sm'>
                  &nbsp;
                </div>
              )
            }
            { (currentWarning === '')? (<div/>) : (
                <div className='p-4 mx-4 mb-4 text-sm text-yellow-700 bg-yellow-100 rounded-lg' role='alert'>
                  <span className='font-semibold'>Warning!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentWarning}
                </div>
              )
            }
            { (currentError === '')? (<div/>) : (
                <div className='p-4 mx-4 mb-4 text-sm text-red-700 bg-red-100 rounded-lg' role='alert'>
                  <span className='font-semibold'>Error!</span>&nbsp;&nbsp;&nbsp;&nbsp;{currentError}
                </div>
              )
            }
            <div className='flex flex-col mb-8 space-y-3 px-4'>
              <span className='text-sm text-light_text_l'>By clicking submit, you agree to the <button type='button' data-modal-toggle='terms' className='text-light_hl underline'>terms of service</button>.</span> 
              {
                (!submitting)? (
                  <button className='bg-light_hl w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded-lg
                                 hover:bg-light_hl_l hover:scale-105
                                 transition-all duration-200'
                          type='submit' disabled={submitting}>
                    Submit
                  </button>
                ) : (
                  <button className='bg-light_disabled w-fit px-5 py-2 text-light_hl_subtext font-bold text-2xl rounded-lg
                                 transition-all duration-200'
                          type='submit' disabled>
                    Processing...
                  </button>
                )
              }
            </div>
          </form>


          <div id='terms' aria-hidden='true' className='hidden overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 w-full md:inset-0 h-modal md:h-full'>
            <div className='relative p-4 w-full max-w-2xl h-auto'>
              <div className='relative bg-white rounded-lg shadow'>
                <div className='flex justify-between items-start p-5 rounded-t-lg border-b'>
                  <h3 className='text-xl font-semibold '>
                    Terms of Service
                  </h3>
                  <button type='button' className='text-light_disabled bg-transparent transition-all duration-200 hover:bg-gray-200 hover: rounded-lg text-sm p-1.5 ml-auto inline-flex items-center' data-modal-toggle='terms'>
                    <svg className='w-5 h-5' fill='currentColor' viewBox='0 0 20 20' xmlns='http://www.w3.org/2000/svg'><path fillRule='evenodd' d='M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z' clipRule='evenodd'></path></svg>  
                  </button>
                </div>
                <div className='p-6 space-y-6'>
                  <p className='text-base leading-relaxed text-light_text_l'>
                    {dialogues.terms}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div id='complete' aria-hidden='true' className='hidden overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 w-full md:inset-0 h-modal md:h-full'>
            <div className='relative p-4 w-full max-w-2xl h-auto'>
              <div className='relative bg-white rounded-lg shadow'>
                <div className='flex justify-between items-start p-5 rounded-t-lg border-b'>
                  <h3 className='text-xl font-semibold '>
                    Success!
                  </h3>
                  <button type='button' className='text-light_disabled bg-transparent transition-all duration-200 hover:bg-gray-200 hover: rounded-lg text-sm p-1.5 ml-auto inline-flex items-center' onClick={closeComplete}>
                    <svg className='w-5 h-5' fill='currentColor' viewBox='0 0 20 20' xmlns='http://www.w3.org/2000/svg'><path fillRule='evenodd' d='M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z' clipRule='evenodd'></path></svg>  
                  </button>
                </div>
                <div className='p-6 space-y-6'>
                  <p className='text-base leading-relaxed text-light_text_l'>
                    A confirmation email has been sent to {senderEmailRaw}. The delivery of the letter will be on the schedule once the confirmation is received.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
