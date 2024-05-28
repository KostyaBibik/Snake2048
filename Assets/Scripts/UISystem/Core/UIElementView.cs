namespace UISystem
{
    public abstract class UIElementView<T> : UIElementBase
    {
        //TODO: Добавить проверку на то, что элемент был инициализирован во избежании ошибок
        public void InvokeUpdateView(T model)
        {
            UpdateView(model);
        }
        
        /// <summary>
        /// Обновление view. Не реккомендуется вызывать из оверрайда метода Initialization
        /// элементов более низкого или того же уровня вложенности.
        /// </summary>
        /// <param name="model"></param>
        protected abstract void UpdateView(T model);
    }
}